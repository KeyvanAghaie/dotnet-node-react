const http = require('http');
const config = require('../config');

/**
 * Helper function to make HTTP requests to dotNet backend
 * @param {string} path - API endpoint path
 * @param {object} options - Request options (method, headers, body)
 * @returns {Promise} - Resolves with response data or rejects with error
 */
function makeRequest(path, options = {}) {
  return new Promise((resolve, reject) => {
    const url = new URL(path, config.GO_BACKEND_URL);
    const requestOptions = {
      hostname: url.hostname,
      port: url.port || 8080,
      path: url.pathname + url.search,
      method: options.method || 'GET',
      headers: {
        'Content-Type': 'application/json',
        ...options.headers
      }
    };

    const req = http.request(requestOptions, (res) => {
      let data = '';
      res.on('data', (chunk) => {
        data += chunk;
      });
      res.on('end', () => {
        if (res.statusCode >= 200 && res.statusCode < 300) {
          try {
            resolve(JSON.parse(data));
          } catch (e) {
            resolve(data);
          }
        } else {
          // Try to parse error response as JSON
          let errorMessage = data;
          try {
            const errorData = JSON.parse(data);
            errorMessage = errorData.error || errorData.message || data;
          } catch (e) {
            // Keep original error message if not JSON
          }
          const error = new Error(errorMessage);
          error.statusCode = res.statusCode;
          error.responseData = data;
          reject(error);
        }
      });
    });

    req.on('error', (error) => {
      reject(error);
    });

    if (options.body) {
      req.write(JSON.stringify(options.body));
    }

    req.end();
  });
}

module.exports = { makeRequest };

const express = require('express');
const router = express.Router();

const healthRoutes = require('./health');
const userRoutes = require('./users');
const taskRoutes = require('./tasks');
const statsRoutes = require('./stats');
const productRoutes = require('./products');

// Mount routes
router.use('/', healthRoutes);
router.use('/api/users', userRoutes);
router.use('/api/tasks', taskRoutes);
router.use('/api/stats', statsRoutes);
router.use('/api/products', productRoutes);


module.exports = router;

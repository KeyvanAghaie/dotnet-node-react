// server/routes/productRoutes.js
const express = require('express');
const router = express.Router();
const multer = require('multer');
const authMiddleware = require('../middleware/errorHandler')
const productController = require('../controllers/productController');

const storage = multer.diskStorage({
    destination: function (req, file, cb) {
      cb(null, 'uploads/');
    },
    filename: function (req, file, cb) {
      cb(null, Date.now() + '-' + file.originalname);
    },
  });
  
const upload = multer({ storage: storage });
  
router.post('/create', upload.single('image'), productController.createProduct);

router.get('/getAll', productController.getAllProducts);

router.post('/:productId/place-bid', productController.placeBid);
router.get('/:productId/bid-history', productController.getBidHistory);

// router.get('/inventory', productController.getAllProducts);
router.put('/update/:productId', productController.updateProduct);

module.exports = router;

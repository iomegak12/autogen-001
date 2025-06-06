PRODUCT_CATALOG = {
    "Dell XPS 15": {"price": 1500, "stock": 10},
    "Apple iPhone 15 Pro": {"price": 1200, "stock": 15},
    "Sony WH-1000XM5 Headphones": {"price": 350, "stock": 20},
    "Samsung Galaxy Tab S9": {"price": 900, "stock": 12},
    "Logitech MX Master 3S Mouse": {"price": 100, "stock": 30},
}

# Sample order database
ORDERS_DB = {
    "ORD-1": {"product": "Dell XPS 15", "quantity": 1, "customer_id": "CUST-001", "status": "Shipped"},
    "ORD-2": {"product": "Apple iPhone 15 Pro", "quantity": 2, "customer_id": "CUST-002", "status": "Processing"},
}

# Sample complaint database
COMPLAINTS_DB = {
    "CMP-1": {"order_id": "ORD-1", "customer_id": "CUST-001", "complaint": "Received a defective product.", "status": "Resolved"},
    "CMP-2": {"order_id": "ORD-2", "customer_id": "CUST-002", "complaint": "Order delayed.", "status": "Pending"},
}

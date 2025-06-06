from db import PRODUCT_CATALOG, ORDERS_DB, COMPLAINTS_DB


def find_closest_product(product_name: str) -> str:
    """Find the closest matching product from the catalog based on substring search."""
    for product in PRODUCT_CATALOG.keys():
        if product_name.lower() in product.lower():
            return product
    return None

# Product inquiry tool


def product_inquiry_tool(product_name: str) -> str:
    """Check product information using product name"""
    closest_product = find_closest_product(product_name)
    if closest_product:
        product = PRODUCT_CATALOG[closest_product]
        return f"{closest_product}: Price = ${product['price']}, Stock = {product['stock']} units."
    return f"Sorry, {product_name} is not available in our catalog."

# Order placement tool


def order_placement_tool(product_name: str, quantity: int = None, customer_id: str = "Guest") -> str:
    """Place order"""
    closest_product = find_closest_product(product_name)
    if not closest_product:
        return f"Sorry, {product_name} is not available."

    if quantity is None:
        return "MISSING_INFO: Please provide the quantity to place your order."

    product = PRODUCT_CATALOG[closest_product]
    if product["stock"] < quantity:
        return f"Only {product['stock']} units of {closest_product} are available."

    # Deduct stock and generate order ID
    product["stock"] -= quantity
    order_id = f"ORD-{len(ORDERS_DB) + 1}"
    ORDERS_DB[order_id] = {"product": closest_product, "quantity": quantity,
                           "customer_id": customer_id, "status": "Processing"}

    return f"Order placed successfully! Order ID: {order_id}"

# Order status tool


def order_status_tool(order_id: str) -> str:
    """Check order status"""
    order = ORDERS_DB.get(order_id)
    if order:
        return f"Order ID: {order_id}, Product: {order['product']}, Quantity: {order['quantity']}, Status: {order['status']}."
    return "Invalid Order ID."

# Complaint registration tool


def complaint_registration_tool(order_id: str, complaint_text: str, customer_id: str = "Guest") -> str:
    """Register complaint"""
    if order_id not in ORDERS_DB:
        return "Invalid Order ID. Cannot register complaint."

    complaint_id = f"CMP-{len(COMPLAINTS_DB) + 1}"
    COMPLAINTS_DB[complaint_id] = {
        "order_id": order_id, "customer_id": customer_id, "complaint": complaint_text, "status": "Pending"}

    return f"Complaint registered successfully! Complaint ID: {complaint_id}"

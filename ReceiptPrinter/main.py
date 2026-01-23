from fastapi import FastAPI
from DTOs.PrintOrderDto import PrintOrderDto
from utilities.format_order import format_order
from utilities.print_order import print_receipt

app = FastAPI()

@app.get("/")
def root():
  return {"Hello": "World"}

@app.post("/print-order")
def print_order(order: PrintOrderDto):
  try:
    # Format the order into 48-character lines
    formatted_lines = format_order(order)
    
    # Print the receipt (handles opening and closing printer)
    print_receipt(formatted_lines)
    
    return {"message": "Order printed successfully", "order_id": order.OrderId}
  except Exception as e:
    return {"message": "Error printing order", "error": str(e), "order_id": order.OrderId}

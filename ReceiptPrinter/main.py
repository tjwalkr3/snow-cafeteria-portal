from fastapi import FastAPI
from DTOs.PrintOrderDto import PrintOrderDto
from utilities.format_order import format_order
from utilities.print_order import open_printer, print_lines

app = FastAPI()

@app.get("/")
def root():
  return {"Hello": "World"}

@app.post("/print-order")
def print_order(order: PrintOrderDto):
  try:
    # Format the order into 48-character lines
    formatted_lines = format_order(order)
    
    # Open the printer and print the lines
    printer = open_printer()
    print_lines(printer, formatted_lines)
    
    return {"message": "Order printed successfully", "order_id": order.OrderId}
  except Exception as e:
    return {"message": "Error printing order", "error": str(e), "order_id": order.OrderId}

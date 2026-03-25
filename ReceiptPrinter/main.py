from fastapi import FastAPI, HTTPException, Request
from pydantic import BaseModel
from DTOs.BrowserOrder import BrowserOrder
from utilities.format_order import format_order
from utilities.print_order import print_receipt

app = FastAPI()


class PrintOrderRequest(BaseModel):
    browserOrder: BrowserOrder
    orderId: int


@app.get("/")
def root():
    return {"Hello": "World"}


@app.post("/print-order")
async def print_order(request: Request):
    body = await request.body()
    print(f"Received JSON: {body.decode('utf-8')}")

    try:
        print_request = PrintOrderRequest.model_validate_json(body)
        formatted_lines = format_order(
            print_request.browserOrder, print_request.orderId
        )
        print_receipt(formatted_lines)
        return {
            "message": "Order printed successfully",
            "user_name": print_request.browserOrder.userName,
        }
    except Exception as e:
        print(f"Error: {str(e)}")
        raise HTTPException(status_code=500, detail=f"Error printing order: {str(e)}")

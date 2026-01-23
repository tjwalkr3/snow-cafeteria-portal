from pydantic import BaseModel
from typing import Optional


class FoodItemOptionDto(BaseModel):
    Id: int
    FoodItemOrderId: int
    FoodOptionName: Optional[str] = None

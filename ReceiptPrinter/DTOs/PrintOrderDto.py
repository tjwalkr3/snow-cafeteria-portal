from pydantic import BaseModel
from typing import List
from datetime import datetime
from .FoodItemDto import FoodItemDto


class PrintOrderDto(BaseModel):
    Id: int
    OrderTime: datetime
    FoodItems: List[FoodItemDto] = []

from pydantic import BaseModel
from typing import Optional


class FoodItemOptionDto(BaseModel):
    Id: int
    FoodItemId: int
    FoodOptionName: Optional[str] = None

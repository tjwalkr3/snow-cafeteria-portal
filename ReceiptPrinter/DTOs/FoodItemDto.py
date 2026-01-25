from pydantic import BaseModel
from typing import Optional, List
from .FoodItemOptionDto import FoodItemOptionDto


class FoodItemDto(BaseModel):
    Id: int
    Name: str = ""
    OrderId: Optional[int] = None
    StationId: Optional[int] = None
    SaleCardId: Optional[int] = None
    SaleSwipeId: Optional[int] = None
    SwipeCost: Optional[int] = None
    CardCost: Optional[float] = None
    Special: Optional[bool] = False
    Options: List[FoodItemOptionDto] = []

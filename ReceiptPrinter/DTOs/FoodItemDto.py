from pydantic import BaseModel
from typing import Optional, List
from .FoodItemOptionDto import FoodItemOptionDto


class FoodItemDto(BaseModel):
    id: int
    name: str = ""
    orderId: Optional[int] = None
    stationId: Optional[int] = None
    saleCardId: Optional[int] = None
    saleSwipeId: Optional[int] = None
    swipeCost: Optional[int] = None
    cardCost: Optional[float] = None
    special: Optional[bool] = False
    options: List[FoodItemOptionDto] = []

from pydantic import BaseModel
from typing import Optional
from decimal import Decimal


class FoodOptionTypeDto(BaseModel):
    id: int = 0
    foodOptionTypeName: str = ""
    requiredAmount: int = 0
    includedAmount: int = 0
    maxAmount: int = 0
    foodOptionPrice: Decimal = Decimal("0")
    entreeId: Optional[int] = None
    sideId: Optional[int] = None
    iconId: Optional[int] = None
    iconBootstrapName: Optional[str] = None

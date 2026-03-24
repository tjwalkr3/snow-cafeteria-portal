from pydantic import BaseModel
from typing import Optional
from decimal import Decimal


class DrinkDto(BaseModel):
    id: int = 0
    locationId: int = 0
    drinkName: str = ""
    drinkDescription: Optional[str] = None
    drinkPrice: Decimal = Decimal("0")
    imageUrl: Optional[str] = None
    inStock: bool = True

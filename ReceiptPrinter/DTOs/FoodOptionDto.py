from pydantic import BaseModel
from typing import Optional


class FoodOptionDto(BaseModel):
    id: int = 0
    foodOptionName: str = ""
    inStock: bool = True
    imageUrl: Optional[str] = None

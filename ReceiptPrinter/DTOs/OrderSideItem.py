from pydantic import BaseModel
from typing import List
from .SideDto import SideDto
from .SelectedFoodOption import SelectedFoodOption


class OrderSideItem(BaseModel):
    side: SideDto = SideDto()
    selectedOptions: List[SelectedFoodOption] = []

from pydantic import BaseModel
from typing import List
from .EntreeDto import EntreeDto
from .SelectedFoodOption import SelectedFoodOption


class OrderEntreeItem(BaseModel):
    entree: EntreeDto = EntreeDto()
    selectedOptions: List[SelectedFoodOption] = []

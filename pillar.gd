extends Sprite2D

## The tilemap that holds the pillar's collision tiles.
@export var tile_map: TileMapLayer
## Tile coordinates that make up the pillar's collision.
@export var pillar_cells: Array[Vector2i] = [
	Vector2i(13, 2), Vector2i(14, 2),
	Vector2i(13, 3), Vector2i(14, 3),
]
## Pillar base tiles: replaced with plain flat ground once the pillar is gone.
@export var base_cells: Array[Vector2i] = [Vector2i(13, 4), Vector2i(14, 4)]
@export var ground_atlas_coords := Vector2i(1, 3)

func remove() -> void:
	if tile_map:
		for cell in pillar_cells:
			tile_map.erase_cell(cell)
		for cell in base_cells:
			tile_map.set_cell(cell, 0, ground_atlas_coords)
		tile_map.update_internals()
	queue_free()

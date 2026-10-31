extends Area2D

## The column/obstacle that disappears when the button is pressed.
@export var column: Node2D
@export var push_distance: float = 10.0

var pressed := false

func _ready() -> void:
	body_entered.connect(_on_body_entered)

func _on_body_entered(body: Node2D) -> void:
	if not body.is_in_group("player"):
		return
	if pressed:
		return
	pressed = true
	# Defer: we're inside a physics callback, so don't change collision state directly.
	_press.call_deferred()

func _press() -> void:
	position.x -= push_distance
	if column:
		if column.has_method("remove"):
			column.remove()
		else:
			column.visible = false

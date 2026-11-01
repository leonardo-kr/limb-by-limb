shader_type canvas_item;

// Positions are fractions of the image width (0 = left edge, 1 = right edge).
// Left fade: transparent at left_start, fully visible at left_end. Leave both at 0 to disable.
uniform float left_start = 0.0;
uniform float left_end = 0.0;
// Right fade: fully visible at right_start, transparent at right_end. Leave both at 1 to disable.
uniform float right_start = 1.0;
uniform float right_end = 1.0;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
    float alpha = 1.0;
    if (left_end > left_start) {
            alpha *= smoothstep(left_start, left_end, UV.x);
    }
    if (right_end > right_start) {
            alpha *= 1.0 - smoothstep(right_start, right_end, UV.x);
    }
    COLOR.a *= alpha;
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}

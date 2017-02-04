#version 410

//http://ogldev.atspace.co.uk/www/tutorial28/tutorial28.html

layout(location=0) in vec3 Position;
/*
layout(location=1) in vec3 Velocity;
layout(location=2) in float Size;
layout(location=3) in float Age;
*/
out vec3 Position0;
/*
out vec3 Velocity0;
out float Size0;
out float Age0;
*/
void main()
{
	Position0 = Position;
/*
	Velocity0 = Velocity;
	Size0 = Size;
	Age0 = Age;
*/
} 

#version 410
//http://ogldev.atspace.co.uk/www/tutorial28/tutorial28.html

layout(points) in;
layout(points) out;
layout(max_vertices = 3000) out;

in vec3 Position0[];
/*
in vec3 Velocity0[];
in float Size0[];
in float Age0[];
*/
out vec3 Position1;
/*
out vec3 Velocity1;
out float Size1;
out float Age1;
*/
void main()
{	
	Position1 = Position0[0] + vec3(0.001, 0.001, 0.001);	
    EmitVertex();
    EndPrimitive();
} 
#version 330
//http://ogldev.atspace.co.uk/www/tutorial28/tutorial28.html

layout(points) in;
layout(triangle_strip, max_vertices = 30) out;

in float Sprite[];
in float Age[];

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

out float fSprite;
out float fAge;
out vec2 texCoord;

void main()
{
	fSprite = Sprite[0];
	fAge = Age[0];

	mat4 transform = projection * view * model;

	vec4 center = gl_in[0].gl_Position;
	
	gl_Position = transform * (center + vec4(-0.1, 0.1, 0.0, 1.0));
	texCoord = vec2(0.0, 0.0);
	EmitVertex();
    gl_Position = transform * (center + vec4(0.1, 0.1, 0.0, 1.0));
    texCoord = vec2(1.0, 0.0);
	EmitVertex();
	gl_Position = transform * (center + vec4(0.1, -0.1, 0.0, 1.0));
	texCoord = vec2(1.0, 1.0);
    EmitVertex();
	gl_Position = transform * (center + vec4(-0.1, -0.1, 0.0, 1.0));
	texCoord = vec2(0.0, 1.0);
    EmitVertex();
	gl_Position = transform * (center + vec4(-0.1, 0.1, 0.0, 1.0));
	texCoord = vec2(0.0, 0.0);
	EmitVertex();
    EndPrimitive();
} 
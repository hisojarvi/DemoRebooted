#version 330
//http://ogldev.atspace.co.uk/www/tutorial28/tutorial28.html

#define PI 3.1415926535897932384626433832795

layout(points) in;
layout(triangle_strip, max_vertices = 30) out;

in float Sprite[];
in float Age[];
in float Size[];
in float Rotation[];

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

out float fSprite;
out float fAge;
out vec2 texCoord;

float SizeAtAge()
{
	float sizes[5] = float[](0.8, 1.2, 1.0, 0.8, 0.2);
	int index1 = int(floor(fAge*5));
	int index2 = index1 + 1;
	float blendFactor = fract(fAge*5);
	return mix(sizes[index1], sizes[index2], blendFactor);
}



void main()
{
	fSprite = Sprite[0];
	fAge = Age[0];

	mat4 transform = projection * view * model;

	vec4 center = gl_in[0].gl_Position;

	float rotation = Rotation[0];

	float size = Size[0] * SizeAtAge();
	vec4 topLeft = gl_in[0].gl_Position + vec4(size * vec3(cos(rotation), sin(rotation), 0.0), 1.0);
	vec4 topRight = gl_in[0].gl_Position + vec4(size * vec3(cos(rotation+PI/2), sin(rotation+PI/2), 0.0), 1.0);
	vec4 bottomRight = gl_in[0].gl_Position + vec4(size * vec3(cos(rotation-PI/2), sin(rotation-PI/2), 0.0), 1.0);
	vec4 bottomLeft = gl_in[0].gl_Position + vec4(size * vec3(cos(rotation-PI), sin(rotation-PI), 0.0), 1.0);

	gl_Position = transform * topLeft;
	texCoord = vec2(0.0, 0.0);
	EmitVertex();
    gl_Position = transform * topRight;
    texCoord = vec2(1.0, 0.0);
	EmitVertex();
	gl_Position = transform * bottomRight;
	texCoord = vec2(1.0, 1.0);
    EmitVertex();
	gl_Position = transform * bottomLeft;
	texCoord = vec2(0.0, 1.0);
    EmitVertex();
	gl_Position = transform * topLeft;
	texCoord = vec2(0.0, 0.0);
	EmitVertex();
    EndPrimitive();
} 
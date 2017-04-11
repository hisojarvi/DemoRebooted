#version 410

layout(location=0) in vec3 position;
layout(location=1) in float sprite;
layout(location=2) in float age;
layout(location=3) in float maxage;
layout(location=4) in float size;
layout(location=5) in float rotation;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

out float Sprite;
out float Age;
out float Size;
out float Rotation;

void main()
{
	gl_Position = vec4(position, 1.0);
	Sprite = sprite;
	Age = age/maxage;
	Size = size;
	Rotation = rotation;
}
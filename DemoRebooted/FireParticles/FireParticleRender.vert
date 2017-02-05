#version 410

layout(location=0) in vec3 position;
layout(location=1) in float sprite;
layout(location=2) in float age;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

out float Sprite;
out float Age;

void main()
{
	gl_Position = vec4(position, 1.0);
	Sprite = sprite;
	Age = age;
}
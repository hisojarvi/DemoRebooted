#version 410

layout(location=0) in vec3 position;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

out vec3 Position;

void main()
{
	gl_Position = vec4(position, 1.0);
}
#version 150

in vec3 position;
in vec2 texcoord;
in vec3 normal;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

out vec2 Texcoord;
out vec4 Normal;

void main()
{
    Texcoord = texcoord;
	gl_Position = projection * view * model * vec4(position, 1.0);
	Normal = projection * view * model * vec4(normal, 1.0);
}
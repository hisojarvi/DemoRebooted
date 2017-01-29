#version 150

in vec3 position;
in vec2 texcoord;
in vec3 normal;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

out vec3 Position;
out vec2 Texcoord;
out vec3 Normal;

void main()
{
    Texcoord = texcoord;
	gl_Position = projection * view * model * vec4(position, 1.0);
	mat3 normMatrix = transpose(inverse(mat3(model)));
	Normal = normMatrix * normal;
	Position = (model * vec4(position, 1.0)).xyz;
}
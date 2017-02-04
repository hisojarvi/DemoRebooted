#version 330
//http://ogldev.atspace.co.uk/www/tutorial28/tutorial28.html

layout(points) in;
layout(triangle_strip, max_vertices = 30) out;



void main()
{
/*
	gl_Position = projection * view * model * vec4(position, 1.0);
*/
	vec4 center = gl_in[0].gl_Position;
	gl_Position = center + 0.1*vec4(-0.1, 0.1, 0.0, 1.0);
	EmitVertex();
    gl_Position = center + 0.1*vec4(0.1, 0.1, 0.0, 1.0);
    EmitVertex();
	gl_Position = center + 0.1*vec4(0.1, -0.1, 0.0, 1.0);
    EmitVertex();
	gl_Position = center + 0.1*vec4(-0.1, -0.1, 0.0, 1.0);
    EmitVertex();
	gl_Position = center + 0.1*vec4(-0.1, 0.1, 0.0, 1.0);
	EmitVertex();
    EndPrimitive();


} 
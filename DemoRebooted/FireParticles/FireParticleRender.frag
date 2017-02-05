#version 330

in float fSprite;
in float fAge;
in vec2 texCoord;

uniform sampler2D texFireSprites;
uniform sampler2D texFireSpritesPalette;
uniform int numSprites;

out vec4 outColor;

float frame()
{
	float cycleDuration = 1.0;
	float frameLength = cycleDuration / numSprites;
	int frameNonWrapped = int(floor(fAge/frameLength));
	return mod(frameNonWrapped, numSprites);
}

void main()
{
	float x = fSprite/numSprites + texCoord.x / numSprites;
	vec2 coord = vec2(x, texCoord.y);

	vec4 outLuminosity = texture2D(texFireSprites, coord);
	outColor = texture2D(texFireSpritesPalette, vec2(outLuminosity.r, 0.0));

	outColor.a = outLuminosity.r;
}
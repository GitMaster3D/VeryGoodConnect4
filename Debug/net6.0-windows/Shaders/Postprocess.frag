#version 330 core

out vec4 outputColor;


in vec2 uv;
uniform sampler2D texture0;

const float bloomThreshold = 0.7f;  // Threshold to determine bright pixels
const float bloomIntensity = 3;  // Intensity of the bloom effect



void main()
{
    // Sample the texture color
    vec4 textureColor = texture(texture0, uv);
    
    // Determine the brightness of the pixel
    float brightness = max(max(textureColor.r, textureColor.g), textureColor.b);
    
    // Apply bloom if the pixel is bright enough
    if (brightness > bloomThreshold)
    {
        // Calculate the bloom color
        vec4 bloomColor = (brightness - bloomThreshold) * bloomIntensity * textureColor;
        
        // Add the bloom color to the original color
        textureColor += bloomColor;
    }
    
    // Output the final color
    outputColor = textureColor;
}



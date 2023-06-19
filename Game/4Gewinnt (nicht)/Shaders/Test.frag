#version 330 core

out vec4 outputColor;

float shadowBias = 0.05f;

in vec2 texCoord;
in vec3 normal;
in vec3 fragPos;

uniform float time;

uniform sampler2D texture0;
uniform sampler2D shadowMap;


uniform vec3 lightColor;
uniform vec3 lightPos;


uniform float alpha;

uniform vec3 viewPos;

struct Material
{
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float shininess;
};
uniform Material material;





float LinearizeDepth(float depth)
{
    float near = 0.1f;
    float far = 100;

    return (2.0 * near) / (far + near - depth * (far - near));
}
float CalculateShadow(float depthValue)
{
    float shadow = 1;
    depthValue = LinearizeDepth(depthValue);

    if (depthValue > depthValue + shadowBias)
    {
        shadow = 0.5f;
    }
    return shadow;
}



void main()
{

    // Diffuse Lighting
    vec3 ambient = lightColor * material.ambient;

    vec3 norm = normalize(normal);
    vec3 lightDir = normalize(lightPos - fragPos);

    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = (diff * material.diffuse) * lightColor;

    // Specular Lighting
    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = (material.specular * spec) * lightColor;

    vec3 result = ambient + diffuse + specular;

    // Shadow calculation
    float shadowFactor = 1.0f;
    float depthValue = texture(shadowMap, texCoord).r;

    float shadow = CalculateShadow(depthValue + shadowBias);

    vec4 finalColor = texture(texture0, texCoord) * vec4(result, 1.0) * vec4(1.0, 1.0, 1.0, alpha);
    finalColor.rgb *= shadow;
    outputColor = finalColor;
}

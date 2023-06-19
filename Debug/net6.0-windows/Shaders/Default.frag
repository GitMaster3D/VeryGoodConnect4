#version 330 core

out vec4 outputColor;

in vec2 texCoord;
in vec3 normal;
in vec3 fragPos;

uniform float time;

uniform sampler2D texture0;


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


void main()
{
    vec3 ambient = lightColor * material.ambient;

    vec3 norm = normalize(normal);
    vec3 lightDir = normalize(lightPos - fragPos);

    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = (diff * material.diffuse) * lightColor;

    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = (material.specular * spec) * lightColor;

    vec3 result = ambient + diffuse + specular;

    outputColor = vec4(result, alpha) * texture(texture0, texCoord);
}

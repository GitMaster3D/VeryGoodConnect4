#version 330 core
#extension GL_ARB_separate_shader_objects : enable

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;
layout(location = 2) in vec3 aNormal;

out vec2 texCoord;
out vec3 normal;
out vec3 fragPos;


uniform mat4 transform;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    gl_Position = vec4(aPosition, 1.0) * transform * view * projection;

    texCoord = aTexCoord;
    fragPos = vec3(vec4(aPosition, 1.0) * transform);
    normal = aNormal * mat3(transpose(inverse(transform)));
}
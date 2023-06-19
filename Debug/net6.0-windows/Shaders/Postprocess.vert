#version 330 core
#extension GL_ARB_separate_shader_objects : enable

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;

out vec2 uv;

void main()
{
    gl_Position = vec4(aPosition, 1.0f);
    uv = aTexCoord;
}
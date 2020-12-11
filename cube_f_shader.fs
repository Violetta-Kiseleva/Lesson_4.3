#version 330 core

out vec4 FragColor;

struct Material {
    sampler2D diffuse;	// рассеянный свет
    sampler2D specular;	// бликовой свет 
    float shininess;	// сила блеска
}; 
  
struct Light {
    vec3 position;	// не нужен для направленного источника
    //vec3 direction;
    
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    // для уменьшения интенсивности света
    float constant;
    float linear;				
    float quadratic;

    // угол отсечения (радиус подсветки)
    //float cutOff;
    //float outerCutOff;
};

uniform Light light; 
uniform Material material;
uniform vec3 viewPos;		// положение камер

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoords;


// uniform vec3 objectColor;	// цвет объекта
// uniform vec3 lightColor = vec3(1.0)	// цвет источника света (чтобы не засвечивался)
// uniform vec3 lightPos; 
 



//uniform sampler2D ourTexture1;
//uniform sampler2D ourTexture2;

void main()
{    
    // фоновое освещение
    vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));
    
    vec3 norm = normalize(Normal);
    
    vec3 lightDir = normalize(light.position - FragPos);  // вектор направления задается как ориентированный от источника
    // vec3 lightDir = normalize(-light.direction);	// вектор направления от источника (направленный)
    float diff = max(dot(norm, lightDir), 0.0);

    // диффузное освещение
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));  

    // бликовое освещение
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);  
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));  	
   

    /*float theta = dot(lightDir, normalize(-light.direction));
    float epsilon   = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);
    diffuse *= intensity;
    specular *= intensity; */

    // расстояние от источника до объекта
    float distance = length(light.position - FragPos);			

    // коэфф. затухания света
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));	

    // учитываем дальность объекта
    ambient  *= attenuation; 
    diffuse  *= attenuation;				
    specular *= attenuation;

    vec3 result = ambient + diffuse + specular;
    FragColor = vec4(result, 1.0);
}
#define GLI_INCLUDE_EXT_SEPARATE_SHADER_OBJECTS

enum Main {

  //GL_ACTIVE_PROGRAM_EXT         = 0x8B8D, //(alias for CURRENT_PROGRAM)

};


void glUseShaderProgramEXT(GLenum[Main] type, GLuint program);
void glActiveProgramEXT(GLuint program);
GLuint glCreateShaderProgramEXT(GLenum[Main] type, const GLchar *string);

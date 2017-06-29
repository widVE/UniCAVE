#define GLI_LEGACY_INCLUDE_EXT

//Selected legacy extensions
enum Main {

  GL_TEXTURE_TOO_LARGE_EXT            = 0x8065,

};

void glAddSwapHintRectWIN(GLint x, GLint y, GLsizei width, GLsizei height);

// GL_KTX_buffer_region
GLuint glNewBufferRegion(GLenum type);
void glDeleteBufferRegion(GLuint region);
void glReadBufferRegion(GLuint region, GLint x, GLint y, GLsizei width, GLsizei height);
void glDrawBufferRegion(GLuint region, GLint x, GLint y, GLsizei width, GLsizei height, GLint xDest, GLint yDest);
GLuint glBufferRegionEnabled(void);
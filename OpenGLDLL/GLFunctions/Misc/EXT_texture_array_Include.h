#define GLI_INCLUDE_EXT_TEXTURE_ARRAY


enum Main {
  
  //GL_TEXTURE_1D_ARRAY_EXT             = 0x8C18,
  //GL_PROXY_TEXTURE_1D_ARRAY_EXT       = 0x8C19,
  //GL_TEXTURE_2D_ARRAY_EXT             = 0x8C1A,
  //GL_PROXY_TEXTURE_2D_ARRAY_EXT       = 0x8C1B,
  //GL_TEXTURE_BINDING_1D_ARRAY_EXT     = 0x8C1C,
  //GL_TEXTURE_BINDING_2D_ARRAY_EXT     = 0x8C1D,
  //GL_MAX_ARRAY_TEXTURE_LAYERS_EXT     = 0x88FF,
  //GL_COMPARE_REF_DEPTH_TO_TEXTURE_EXT = 0x884E,
/* reuse GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_LAYER_EXT */
};

void glFramebufferTextureLayerEXT(GLenum[Main] target, GLenum[Main] attachment, GLuint texture, GLint level, GLint layer);

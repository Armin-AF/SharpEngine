﻿using System;
using System.IO;
using GLFW;
using static OpenGL.Gl;

namespace SharpEngine
{ 
    struct Vector
    {
        public float x, y, z;

        public Vector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }
        public static Vector operator *(Vector v, float f) {
            return new Vector(v.x * f, v.y * f, v.z * f);
        }
        public static Vector operator +(Vector v, Vector u) {
            return new Vector(v.x + u.x, v.y + u.y, v.z + u.z);
        }
        public static Vector operator -(Vector v, float f) {
            return new Vector(v.x - f, v.y - f, v.z - f);
        }
        public static Vector operator /(Vector v, float f) {
            return new Vector(v.x / f, v.y / f, v.z / f);
        }
        
    }
    
    class Program
    {
       
        static Vector[] vertices = new Vector[] {
          new Vector(-0.1f,-0.1f),
          new Vector(0.1f,-0.1f),
          new Vector(0f,0.1f),
          new Vector(0.4f,0.4f),
          new Vector(0.6f,0.4f),
          new Vector(0.5f,0.6f),
          
        };
        



        static void Main(string[] args) {
            var window = CreateWindow();
            

            LoadTriangleIntoBuffer();

            CreateShaderProgram();
            
            var direction = new Vector(0.0003f, 0.0003f);

            // engine rendering loop
            while (!Glfw.WindowShouldClose(window)) {
                Glfw.PollEvents(); // react to window changes (position etc.)
                glClearColor(.2f, .05f, .2f, 1);
                glClear(GL_COLOR_BUFFER_BIT);
                glDrawArrays(GL_TRIANGLES, 0, vertices.Length);
                Glfw.SwapBuffers(window);
                //glFlush();
                //vertices[4] += 0.001f;

                
                for (var i = 0; i < vertices.Length; i++) {
                    vertices[i] += direction;
                }

                for (var i = 0; i < vertices.Length; i++) {
                    if (vertices[i].x >= 1 || vertices[i].x <= -1) {
                        direction.x *= -1;
                        break;
                    }
                }
                
                for (var i = 0; i < vertices.Length; i++) {
                    if (vertices[i].y >= 1 || vertices[i].y <= -1) {
                        direction.y *= -1;
                        break;
                    }
                }
                


                

                UpdateTriangleBuffer();
            }
        }

        static void CreateShaderProgram() {
            // create vertex shader
            var vertexShader = glCreateShader(GL_VERTEX_SHADER);
            glShaderSource(vertexShader, File.ReadAllText("shaders/screen-coordinates.vert"));
            glCompileShader(vertexShader);

            // create fragment shader
            var fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
            glShaderSource(fragmentShader, File.ReadAllText("shaders/green.frag"));
            glCompileShader(fragmentShader);

            // create shader program - rendering pipeline
            var program = glCreateProgram();
            glAttachShader(program, vertexShader);
            glAttachShader(program, fragmentShader);
            glLinkProgram(program);
            glUseProgram(program);
        }

        static unsafe void LoadTriangleIntoBuffer() {

            // load the vertices into a buffer
            var vertexArray = glGenVertexArray();
            var vertexBuffer = glGenBuffer();
            glBindVertexArray(vertexArray);
            glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
            UpdateTriangleBuffer();
            glVertexAttribPointer(0, 3, GL_FLOAT, false, sizeof(Vector), NULL);

            glEnableVertexAttribArray(0);
        }
        
        static unsafe void UpdateTriangleBuffer() {
            fixed (Vector* vertex = &vertices[0]) {
                glBufferData(GL_ARRAY_BUFFER, sizeof(Vector) * vertices.Length, vertex, GL_DYNAMIC_DRAW);
            }
        }

        static Window CreateWindow() {
            // initialize and configure
            Glfw.Init();
            Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.Decorated, true);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
            Glfw.WindowHint(Hint.OpenglForwardCompatible, Constants.True);
            Glfw.WindowHint(Hint.Doublebuffer, Constants.True);

            // create and launch a window
            var window = Glfw.CreateWindow(1024, 768, "SharpEngine", Monitor.None, Window.None);
            Glfw.MakeContextCurrent(window);
            Import(Glfw.GetProcAddress);
            return window;
        }
    }
}
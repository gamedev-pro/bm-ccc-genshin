import bpy
import os
import platform

if platform.system() == "Windows":
    os.system('cls')
else:
    os.system('clear')
    
for obj in bpy.context.scene.objects:
    print(obj.name)        
    
    if obj.type == 'ARMATURE':
        for bone in obj.data.bones:
            print(bone.name)
                
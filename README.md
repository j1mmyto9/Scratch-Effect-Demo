# Unity Scratch Effect Demo
Scratch Effect Demo in unity

![](images/demo.gif) 

## How to remove dust/fog from an object in Unity with swipe?
- **Objective­:**
   - The Main objective of this Code sample is to create a dust/fog removing effect using Unity. Using, Texture Masking shader, Mask Construction Shader, Render Texture, Camera Masking Script and masking camera, you can create your own effect. Read on to know how and download the source code. It’s free!!
- **Texture Masking Shader:**
   - This shader gives an effect of a layer of dust on the screen. We only need to add dust texture and render texture (as Mask)in material shader.

   ![](images/dust.png) 

- **How to use Shader­:** `Assets/ScratchEffect/Masked.shader`

    - Start by creating a sprite in Hierarchy panel.
    - Add dust image (as Sprite) in this Sprite
    - Add new Material in this sprite and select Texture Masking Shader.
    - In this material, set Dust Texture as Main Texture and Mask as Render Texture.
    - For Render Texture, create a Render texture in project Panel and Select this render
texture in this material.


- **Mask Construction Shader:** `Assets/ScratchEffect/MaskConstruction.shader`
    This shader gives a wiping off dust effect each time the user swipes the screen. To make it happen, we also need to add an eraser image and set it’s colour too.

   ![](images/construction-shader.png) 

    - In Project Panel Create an empty material
    - Select Shader as “MaskConstruction”
    - Select Main Color and Select Erase image.

- **Masking camera­:**

    Masking Camera shows dust on screen as Mask.
    ![](images/camera.png) 

    How to Use Masking Camera? In Hierarchy panel create a camera.

    - In this Camera component as Clear Flag Select ”Don’t clear”
    - Create a layer and name it as “Mask ” and set camera Culling mask select as “Mask”.
    - Set Projection to “Orthographic”. 
    - and its size “5”
    - add “Camera Masking ” script on this camera object

  ![](images/camera-masking.png) 

    - Drag dust object (Sprite) to get the dust effect on Screen.
    - Drag Eraser Material to wipe off the aforementioned sprite and create the effect.


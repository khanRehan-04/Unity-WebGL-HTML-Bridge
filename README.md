# Unity WebGL mobile input fix | HTML input in Unity WebGL

If(using VSCODE)
{
   // Open this README in Preview mode || press Ctrl + Shift + V in Windows to view embedded images.
}


When working on a Unity WebGL project, I ran into a frustrating issue:  
Unity's built-in `TMP_InputField` components do not play well with mobile browsers — especially when the on-screen keyboard appears.

### The Problem:
- The TMP input field gets **obscured by the soft keyboard**, especially in **landscape mode**
- It’s hard or impossible for users to see what they’re typing
- Behavior is **inconsistent across devices, browsers, and OSes**
- Unity provides **limited control** over soft keyboard interactions in WebGL builds

### My Solution:
To fix this, I created a **custom HTML input overlay** that appears when a TMP field is selected.  
This overlay:
- Renders a native HTML `<input>` on top of the Unity canvas
- Automatically focuses and adjusts on mobile devices
- Syncs the input back to Unity through `SendMessage`

This approach results in a smoother, more intuitive typing experience for WebGL users — especially on touch devices(Landscape mode).

## ⚙️ How It Works

#### ✅ Unity Version Used
This setup was built and tested in:  
**Unity 6.0.51f1 (LTS)**

### 🧱 Custom WebGL Template Setup

Unity allows you to use a custom HTML template for WebGL builds.

---

#### 📋 Why a Custom Template?

To gain full control over the HTML, CSS, and JavaScript behavior during runtime, I **duplicated Unity’s default template** and modified it.

---

#### 📁 How to Create a Custom Template

1. **Navigate to the default template location** on your system:
<Unity Editor Folder>/PlaybackEngines/WebGLSupport/BuildTools/WebGLTemplates/Default

3. **Copy the entire `Default` folder** into your project under: Assets/WebGLTemplates/CustomTemplate

![Image](https://github.com/user-attachments/assets/6ca36079-ca33-4da1-bf7d-cff8a353fbbb)

4. Rename the folder if you’d like (e.g., `CustomTemplate`, `KeyBInputTemplate`, etc.)

5. In Unity, go to:  
**Project Settings → Player → Resolution and Presentation → WebGL Template**  
and select your custom template from the dropdown.

![Image](https://github.com/user-attachments/assets/10ae88b5-7e83-44c3-b8e3-5c7bdd335eda)


#

### 🧠 Unity C# Scripts — InputBridge & HtmlInputFieldHandler

These two scripts work together to connect Unity’s `TMP_InputField` components to the HTML input system.

---

#### 🧩 InputBridge.cs
- Acts as the **central controller**
- Sends calls to JavaScript when a TMP field is selected
- Receives the final typed value back from JavaScript
- Keeps track of all active input fields by unique IDs
  
- ![Image](https://github.com/user-attachments/assets/d77237af-eb1d-47f2-bcfc-970a88cdc399)

---

#### 🧩 HtmlInputFieldHandler.cs
- Attach to each TMP input field which you want to communicate over html.
- Handles showing the input via `InputBridge`
- Updates the field text when HTML input is submitted
- Registers its field with a unique `inputId`

![Image](https://github.com/user-attachments/assets/c80e19cc-4078-4bec-bb87-d6307632c472)
---

#### 📂 File Locations
![Image](https://github.com/user-attachments/assets/b19dbbf3-d47a-4516-95f0-0efa64623637)


### 🧠 Inside `index.html` & `.jslib` — HTML Input Logic

Once the WebGL template is set up, the main logic for showing and handling the native HTML `<input>` lives in two places:

---

### 📄 `index.html`

This file handles the **UI and behavior** of the native HTML input overlay.

#### 📦 What Happens:
1. When Unity requests input, the `ShowHtmlInput(...)` JavaScript function is triggered
2. The Unity canvas is hidden, and a styled HTML `<input>` appears
3. The user types directly into the HTML input (fully mobile/browser native)
4. Once finished (Enter or blur), the input is hidden and value sent back to Unity using:
   ```javascript
   unityInstance.SendMessage("InputBridge", "ReturnFromHtmlInput", inputId + "|" + value);

![Image](https://github.com/user-attachments/assets/31e69fea-84a5-40a5-9627-c8e0431ba8f9)

make sure that the .jslib file stays in same hierarchy Assets>Plugins>WebGL> .jslib to prevent any build errors or manually link it.
5. The .jslib file defines a function that Unity can call from C#.
6. It receives data from C# (via [DllImport("__Internal")])
7. Converts it from pointers to strings
8. Then calls the browser-side ShowHtmlInput(...) function.

![Image](https://github.com/user-attachments/assets/b3535af8-9a6f-4b7d-96c3-12ff87a89796)


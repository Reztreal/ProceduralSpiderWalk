# Spider Procedural Walk

Welcome to the **Spider Procedural Walk** repository! This is a unity project that uses [Animation Rigging](https://docs.unity3d.com/Packages/com.unity.animation.rigging@1.3/manual/index.html) package to procedurally animate a spider.

## Demo
![Spider Procedural Walk](./Gif/spider_walk.gif)

## Features

- **Procedural Leg Movement:** The spider's legs move dynamically, responding to the environment and adjusting their positions in real-time.
- **Adaptive Body Positioning:** The spider's body adjusts its height and orientation based on the positions of its legs and the surface it’s walking on.
- **Customizable Step Settings:** Fine-tune the spider's step length, height, and duration to achieve the desired movement style.
- **Real-time Terrain Adaptation:** The script uses raycasting to detect the ground, enabling the spider to walk smoothly over uneven surfaces.

## Getting Started

To integrate this procedural walking system into your Unity project, follow these steps:

1. **Attach the Script:** Add the `Spider` script to your spider GameObject in Unity.
2. **Assign Transforms:** Configure the script by assigning the necessary transforms for the spider's body, leg targets, and orbits.
3. **Customize Settings:** Adjust the step and body settings to match the desired movement behavior for your spider model.

Note that the spider asset you are using should be rigged properly, I've used Two Bone IK Constraint to set up my targets.


## Contributing

Feel free to contribute to this project by opening issues or submitting pull requests. Whether it's bug fixes, feature enhancements, or general improvements, all contributions are welcome!

## Resources

The most useful resource I could find on the topic of Procedural Animation is [this](https://www.youtube.com/watch?v=e6Gjhr1IP6w) video by Codeer.

---

Happy coding! 🕷️

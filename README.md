# Random Road Generator

Random Road Generator is a Unity tool that allows you to create random, road using prefabs. With this tool, you can generate an infinite path with any necessary turns and obstacles. This README file provides an overview of the tool's components and customization options.

## Features

The Random Road Generator consists of three main scripts:

1. **Road Points**: This script automatically creates points on the road to ensure that each road has a start, end, and any required turns. It offers customizable variables in the Unity Editor to adapt to different user needs.

2. **Random Road Generator**: This script generates the random path by assembling prefabs created with the Road Points script. It allows users to make various changes through variables in the Unity Editor. Users can for exa,ple specify the number at which crossroads should be generated, add prefabs to the generator, and determine the number of pieces to generate at once.

3. **Obstacles Spawn**: This script is responsible for placing obstacles on the generated paths. ___Note that it can only be used on straight road pieces.___ The script takes prefabs and positions them on the spawn points along the road. Users can control or randomize the placement of obstacles using customizable variables in the Unity Editor.

## Usage

To use the Random Road Generator tool, follow these steps:

1. Import the provided scripts into your Unity project.

2. Create a road pieces using the **Road Points** script. Adjust the sizing and proportions to your road model and check if all the points are generated correctly.

3. Generate the random road by using the **Random Road Generator** script. Adjust the variables in the Unity Editor to suit your requirements.

4. If desired, use the Obstacles Spawn script to add obstacles to the straight road pieces. Customize the variables in the Unity Editor to control or randomize the placement of obstacles. Road pieces with obstacles are considered by the generator as normaln straight roads.

5. Play the scene in Unity to visualize and interact with the generated road. You can continue to generate new random paths by retriggering the Random Road Generator script.

## Customization

The Random Road Generator tool provides various customization options to tailor the generated roads to your needs. The customizable variables available in the Unity Editor include:

- **Road Points**:
  - Width and Height of the roads (help with generating the points)
  - The amount of points on the curve (on turns)
  - Special offset (if the road isn't a simple block)

- **Random Road Generator**:
  - Crossroad treshold deciding when to spawn a crossroad (to prevent from them spawning all the time)
  - Number of roads generated at once
  - The road pieces made out of prefabs
  - Border size deciding how big is the area for the generator

- **Obstacles Spawn**:
  - Obstacles prefabs made out of prefabs
  - Controlled spawn of the objects or randomized areas

Feel free to modify these variables according to your project requirements to create unique and diverse road networks.

## Support

If you encounter any issues or have any questions regarding the Random Road Generator tool, please feel free to [open an issue](https://github.com/kamuuucka/RandomRoadGenerator/issues) on the GitHub repository page. I will do my best to assist you.

## Contributing

Contributions to the Random Road Generator tool are welcome! If you have any ideas, suggestions, or improvements, please submit a pull request on the GitHub repository page. I appreciate your contributions.

## License

The Random Road Generator tool is licensed under the [MIT License](https://opensource.org/licenses/MIT). You are free to modify, distribute, and use the tool in both commercial and non-commercial projects.

## About

The Random Road Generator tool was developed by [Kamila Matuszak] and is hosted on GitHub at [this repository](https://github.com/kamuuucka/RandomRoadGenerator/tree/main). Feel free to visit the repository for more information and updates.


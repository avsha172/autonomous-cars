# autonomous-cars
unity project I made

this is the build of the main scene of the project:
<a href="https://avsha172.github.io/autonomous-cars/" target="_blank" rel="noopener noreferrer">Unity game</a>

When opening you will see the next screen you will see this screen. Here there are five roads and each road has a set of cars, you can change the parameters of the car's driving. You can choose to focus each road by clicking the dropdown menu and selecting the road you want to enhance. 

![Alt text](https://github.com/avsha172/autonomous-cars/blob/main/readme-res/roads.png)

In the simulation you can change the camera view by pressing the space bar.
The camera will follow the farthest reaching car of the road currently focusing.

<details open>
  <summary>Driving gif</summary>
 
![Alt text](https://github.com/avsha172/autonomous-cars/blob/main/readme-res/sim-driving.gif)

</details>

## cars driving
All the cars drive at a constant speed. if they go out of the road there is a hidden barrier, hitting it will cause them to halt.
There is also another purpose of the barrier. Each car shoots out 5 raycasts from different directions, the raycasts have a range of 20 unity meters(the cars length is  5 unity meters) if they hit the barrier they return the distance they have passed divided by their range, and if they don't hit anything they return 1.
Those raycasts are used as the input of each car's nn. The output of the nn is the steering angle of each car, the output is normalized to between -1 and 1 and there is a steering angle constant.


### The raycasts
![alt text](https://github.com/avsha172/autonomous-cars/blob/main/readme-res/raycasts.JPG)


## game controls
The speed of the cars, the number of cars at each road and the maximum the cars can steer can all be controlled with the slider in the build screen.
There is also a dropdown menu which allows you to look at each road from a closer angle, and if you want to look at a car from that road you can press space, there are 3 options one from above, from 3d person and from 1st person. The camera will attach to the road's best performing car

## cars learning
The neural network has 1 hidden layer, it consists of 30 perceptrons, both the hidden and the output layer have tanh as their activation function.
after all the cars of a road finish driving or by crashing into the barrier or by finishing the track, they are all ordered by how far they have reached on the track and the 75th percentile of cars are kept as they are, all the rest car's nn are or randomly or created by mutating the best performing car's nns or by doing a crossover between 2 cars from the 75th percentile.
Each has an array of points which defines it, so to order the cars by performance I check what is the closest point from that array to that car, and order the cars by the index of that point.

## insights
The tracks in the model are very different and their difficulty varies.Thus, I decided to test the different sets of cars on all the other tracks and I kept track of their performance. With the data I graph the average performance of the 75th percentile from the set of each track. I do this every 5 generations.
The performance of the different sets of cars on that track is graphed and not the other way around; the set that has trained on the track is also tested.

### The graphs

<details>
  <summary>Track 0</summary>
 
  ![alt text](https://github.com/avsha172/autonomous-cars/blob/main/readme-res/track0.png)
 
</details>

<details>
  <summary>Track 1</summary>
 
  ![alt text](https://github.com/avsha172/autonomous-cars/blob/main/readme-res/track1.png)
 
</details>

<details>
  <summary>Track 2</summary>
 
  ![alt text](https://github.com/avsha172/autonomous-cars/blob/main/readme-res/track2.png)
 
</details>

<details>
  <summary>Track 3</summary>
 
  ![alt text](https://github.com/avsha172/autonomous-cars/blob/main/readme-res/track3.png)
 
</details>

<details>
  <summary>Track 4</summary>
 
![alt text](https://github.com/avsha172/autonomous-cars/blob/main/readme-res/track4.png)

</details>

its is important to note that the results that the graphs seem to indcaite should be taken with a grain of salt because its just one test which speed and steering angle I do not have written down. However it is still intersting that some tracks apper to be better suited for learning.

## In the future

I would be interested in adding more tracks and testing how sets of cars that learn from more than one track perform.
I would also be interested in adding the option to change the size of the nn, the number of perceptrons, the number of layers and maybe even the activation function.

These changes are quite simple, but there are more complex changes that would be interesting to see like adding acceleration to the cars, adding lanes or obstacles to the roads and anything else that would make the model closer to the task of driving in our world.




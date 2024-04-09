# autonomous-cars
unity project I made

this is the build of the main scene of the project:
<a href="https://avsha172.github.io/autonomous-cars/" target="_blank" rel="noopener noreferrer">Unity game</a>

when opening you will see thee next screen.

![alt text](https://github.com/avsha172/autonomous-cars/blob/main/readme-res/roads.png)

## cars driving
All the cars drive at a constent speed. if they go out of the road there is a hidden barrier, hitting it will cause them to halt.
There is also another purpose of the barrier. each car shoots out 5 raycasts from different direction, the raycasts have a range of 20 unity meters(the cars length is  5 unity meters) if they hit the barrier they return the distancethey have passed divided by their range, and if they dont hit anything they return 1.
Those raycasts are used as the input of each car's nn. The output of the nn is the steering angle of each car, the output is normalized to between -1 and 1 and there is a steering angle constent.


# The raycasts
![alt text](https://github.com/avsha172/autonomous-cars/blob/main/readme-res/raycasts.JPG)


## game controlls
The speed of the cars, the number of cars at each road and the maximum the cars can steer can all be controlled with the slider in the build screen.
Also there is a dropdown menu that allows to look at each road in a from a closer angle, and if you want to look at a car from that road you can press space, there are 3 options one from above, from 3d person and from 1st person. The camera will attach to the road's best preforming car

## cars learning
The neural network has 1 hidden layer, it consistes of 30 perceptrons, in both the hidden and the output layer the the activation function is tanh.
after all the cars of a road finish driving or by crashing into the barrier or by finishing the track, they are all ordered by how far they have reached on the track and the 75th percentile of cars are kept as they are, all the rest car's nn are or randomly or created by mutating the best performing car's nns or by doing crossover between 2 nns.
Each has an array of points which defines it, so to order the cars by performance I check what is the closest point from that array to that car, and order the cars by the index of that point.

## insights
The tracks in the model are very different and their difficulty varies.Thus, i decided to test the different sets of cars on all the other tracks and i kept track of their performance. With the data i saved i graphed the avrage performance of the 75th percentile from the set of each track. I do this every 5 generations.
The performance of the different sets of cars on that track is graphed and not the otherway around; the set that has trained on the track is also tested.
# The graphs

![alt text](https://github.com/avsha172/autonomous-cars/blob/main/readme-res/track0.png)
![alt text](https://github.com/avsha172/autonomous-cars/blob/main/readme-res/track1.png)
![alt text](https://github.com/avsha172/autonomous-cars/blob/main/readme-res/track2.png)
![alt text](https://github.com/avsha172/autonomous-cars/blob/main/readme-res/track3.png)
![alt text](https://github.com/avsha172/autonomous-cars/blob/main/readme-res/track4.png)

# autonomous-cars
unity project I made

this is the build of the main scene of the project:
<a href="https://avsha172.github.io/autonomous-cars/" target="_blank" rel="noopener noreferrer">Unity game</a>

when opening you will see thee next screen.

![alt text](https://github.com/avsha172/autonomous-cars/blob/main/readme-res/roads.png)

## cars driving
All the cars drive at a constent speed. if they go out of the road there is a hidden barrier there that when hitting it they will stop driving.
There is also another purpose of the barrier. each car shoots out 5 raycasts from different direction, the raycasts have a range of 20 unity meters(the cars length is  5 unity meters) if they hit the barrier they return the distancethey have passed divided by their range, and if they dont hit anything they return 1.
Those raycasts are used as the input of each car's nn. The output of the nn is the steering angle of each car, the output is normalized to between -1 and 1 and there is a steering angle constent.

The raycasts are shoot each frame, each from a different direction, the first is from the left, which is angle 0; and then 45; 90 ,whcih is infront of the car; 135; and 180, which is to the right of the car.

## game controlls
The speed of the cars, the number of cars at each road and the maximum the cars can steer can all be controlled with the slider in the build screen.
Also there is a dropdown menu that allows to look at each road in a from a closer angle, and if you want to look at a car from that road you can press space, there are 3 options one from above, from 3d person and from 1st person. The camera will attach to the road's best preforming car

## cars learning
The neural network has 1 hidden layer, it consistes of 30 perceptrons, in both the hidden and the output layer the the activation function is tanh.
after all the cars of a road finish driving or by crashing into the barrier or by finishing the track, they are all ordered by how far they have reached on the track and the 75th percentile of cars are kept as they are, all the rest car's nn are or randomly or created by mutating the best performing car's nns or by doing crossover between 2 nns.
Each has an array of points which defines it, so to order the cars by performance I check what is the closest point from that array to that car, and order the cars by the index of that point.

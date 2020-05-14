# Unity-ML-Agent-Car-prototype

![thumbnail](/Images/thumbnail.png)

[Youtube showcase](https://www.youtube.com/watch?v=57sL9KRN-2s)

Small prototype containing trained car brains and different car controllers using wheel colliders, HingeJoints or simple Rigidbodies.
I started with creating different controllable cars. 
Wheel colliders felt too realistic for my taste, it was quite easy to accidentally flip or slide into an U turn, 
I also didn’t fully understand all their settings, so i wrote my own using HingeJoints. 
These however made turning the wheels difficult. Then i went with simple Rigidbodies, 
which didn’t behave as realistic but felt the most fun to control for me. 
I used them to set up an agent that should learn to finish a track i prepared as fast as possible. 
After a lot of training and adjustments I ended up with the following agent reward structure:

● each step: tiny punishment

● braking: less tiny punishment

● going forward: small reward

● going fast: small reward

● entering one of the checkpoints on the track: maximum reward

● entering a checkpoint again: maximum punishment and reset the episode, car is going wrong way

● colliding with a wall: maximum punishment

Its observed values are:

● its own velocity

● distance to next checkpoint

● several raycasts (whether it hit and its distance to the hit)

It still took the cars several hours of training to be able to finish the track, 
but i am happy with the result as the trained brain is actually quite challenging to beat.




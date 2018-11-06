# Send Test Orders

- Open the OrderProducer in swagger: https://robot-orderproducer-dev.azurewebsites.net/swagger
- To create orders, there are two approaches available: ```Orders``` and ```OrderProducer```
    - ```Orders``` allows you to send an order with a given message
    - ```OrderProducer``` creates a stream of orders. The two ```post``` Restful APIs *start* and *stop* the stream. To start the stream, 
        - *maxItems* is the number of orders you want to post, **-1** makes the stream running infinitely
        - *batchSize* is the number of orders you want to post each time, e.g., *maxItems* is 10, *batchSize* is 2, then stream will post 5 iterations of orders with 2 orders each 
        - *delayInSecs* is the interval time between posts

## Send a Job to a Robot

- Open the Dispatcher in swagger: https://robot-dispatcher-dev.azurewebsites.net/swagger
- Create a job for the robot. The id and order id need to be a guid. The RobotId should correspond to one of the robots that you have up, hackbot{0 to n-1}
import matplotlib.pyplot as plt
import numpy as np

x = np.linspace(0, 5, 400)
y1 = x  # for linear transition
y2 = 1 - np.exp(-x)  # for exponential smoothing

plt.figure()
plt.plot(x, y1, label="Linear Transition")
plt.plot(x, y2, label="Exponential Smoothing")
plt.legend()
plt.show()
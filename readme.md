neuromancer network viz
=======================

This is an experimental 3d network traffic visualizer built in Unity and
Node.js. It is inspired by descriptions of hacking in the novel
*[Neuromancer](https://en.wikipedia.org/wiki/Neuromancer)*.

It lacks polish and I'm not sure where to take it next. It could be cool
to turn it into some sort of
hacking/[ICE](https://en.wikipedia.org/wiki/Intrusion_Countermeasures_Electronics) game.

## Setup
You will need a copy of [Unity](http://unity3d.com/) (the free edition
is fine).

You'll also need:

* [node](http://nodejs.org/)
* npm
* libpcap

Clone this repo, and then install the node dependencies:
```bash
$ cd server
$ npm install
```

## Usage
There are two separate components:

* the server (which monitors network
traffic)
* the client (which is the 3D visualization).

While connected to a network, run the server:
```bash
$ sudo node server.js
```
`sudo` is necessary so [node_pcap](https://github.com/mranney/node_pcap)
has the proper permissions.

With the server running, you can open the client in Unity and hit the
play button. You should see the network entities appearing and packets
flowing between them.

![neuromancer exampler](http://spaceandtim.es/uploads/neuro_clip.gif)

## Thanks
Thanks to [Jonathan Dahan](https://twitter.com/jedahan) for his
[pagesounds](https://github.com/jedahan/pagesounds) project which was
the basis for the network-monitoring server.

## License
This code is released under the MIT license.
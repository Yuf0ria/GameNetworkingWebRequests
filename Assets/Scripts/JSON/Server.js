const Server = require('express');
const unity = Server();
app.use(Server.json());

let players = []; // in-memory storage
let nextId = 1;

// Register
app.post('/api/player/register', (req, res) => {
    const { username, password } = req.body;
    if (!username || !password)
        return res.status(400).json({ error: "Username and password required" });
    if (players.find(p => p.username === username))
        return res.status(400).json({ error: "Username already exists" });

    const player = { id: nextId++, username, password, kills: 0, deaths: 0 };
    players.push(player);
    res.status(201).json({ message: "Registered successfully", id: player.id });
});

// Login
app.post('/api/player/login', (req, res) => {
    const { username, password } = req.body;
    const player = players.find(p => p.username === username && p.password === password);
    if (!player)
        return res.status(401).json({ error: "Invalid username or password" });

    res.json({ message: "Login successful", id: player.id, username: player.username });
});

// Get player
app.get('/api/player/:id', (req, res) => {
    const player = players.find(p => p.id === parseInt(req.params.id));
    if (!player)
        return res.status(404).json({ error: "Player not found" });

    res.json({ id: player.id, username: player.username, kills: player.kills, deaths: player.deaths });
});

// Update score
app.put('/api/player/score', (req, res) => {
    const { id, kills, deaths } = req.body;
    const player = players.find(p => p.id === id);
    if (!player)
        return res.status(404).json({ error: "Player not found" });

    player.kills = kills;
    player.deaths = deaths;
    res.json({ message: "Score updated", kills: player.kills, deaths: player.deaths });
});

// Update password
app.put('/api/player/updatePassword', (req, res) => {
    const { id, oldPassword, newPassword } = req.body;
    const player = players.find(p => p.id === id);
    if (!player)
        return res.status(404).json({ error: "Player not found" });
    if (player.password !== oldPassword)
        return res.status(401).json({ error: "Old password incorrect" });

    player.password = newPassword;
    res.json({ message: "Password updated successfully" });
});

// Delete player
app.delete('/api/player/:playerId', (req, res) => {
    const index = players.findIndex(p => p.id === parseInt(req.params.playerId));
    if (index === -1)
        return res.status(404).json({ error: "Player not found" });

    players.splice(index, 1);
    res.json({ message: "Player deleted successfully" });
});

app.listen(3000, () => console.log('Server running on http://localhost:3000'));
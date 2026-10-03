// Client-side bridge to the /auth cookie endpoints in Program.cs.
window.cookieBridge = {

    // Exchanges a signed-in user's Oid for the HttpOnly "uid" cookie. Returns true on success.
    exchangeUser: async function (userOid) {
        try {
            const response = await fetch('/auth/exchange', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ userOid: Number(userOid) })
            });
            return response.ok;
        } catch (error) {
            console.error('Failed to exchange user cookie:', error);
            return false;
        }
    },

    // Asks the server who the "uid" cookie belongs to. Returns { userOid } or null.
    // /auth/whoami answers 204 No Content (not 401) when there is no cookie - an expected
    // "not signed in" outcome with no body, so only a 200 is parsed.
    whoami: async function () {
        try {
            const response = await fetch('/auth/whoami');
            if (response.status === 200) {
                return await response.json();
            }
            return null;
        } catch (error) {
            console.error('Failed to get user info:', error);
            return null;
        }
    },

    // Deletes the "uid" cookie. Returns true on success.
    logout: async function () {
        try {
            const response = await fetch('/auth/logout', { method: 'POST' });
            return response.ok;
        } catch (error) {
            console.error('Failed to clear user cookie:', error);
            return false;
        }
    }
};

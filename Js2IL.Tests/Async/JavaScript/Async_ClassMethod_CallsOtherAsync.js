"use strict";

class Service {
    async fetchData() {
        return await Promise.resolve(100);
    }

    async processData() {
        const data = await this.fetchData();
        return data * 2;
    }
}

const service = new Service();
service.processData().then((result) => {
    console.log("Processed:", result);
});
